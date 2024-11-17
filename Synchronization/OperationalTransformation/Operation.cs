using System.Diagnostics;
using System.Text;

namespace OperationalTransformation;

public enum OperationType
{
    None,
    Insert,
    Delete
}

//See https://en.wikipedia.org/wiki/Operational_transformation for some explanation

public class Operation
{
    public OperationType Type { get; set; }
    public int Pos { get; set; }
    public string Text { get; set; }
    public int UserID { get; set; }

    public Operation() { }

    public Operation(OperationType type, int pos, string text, int userID)
    {
        Type = type;
        Pos = pos;
        Text = text;
        UserID = userID;
    }

    public Operation Transform(Operation op, bool thisOpWins = false)
    {
        if (this.Type == OperationType.None || op.Type == OperationType.None)
        {
            return this;
        }

        if (this.Type == OperationType.Insert)
        {
            if (op.Type == OperationType.Insert)
            {
                return TransformInsertInsert(this, op, thisOpWins);
            }

            if (op.Type == OperationType.Delete)
            {
                return TransformInsertDelete(this, op);
            }
        }

        if (this.Type == OperationType.Delete)
        {
            if (op.Type == OperationType.Insert)
            {
                return TransformDeleteInsert(this, op);
            }

            if (op.Type == OperationType.Delete)
            {
                return TransformDeleteDelete(this, op);
            }
        }

        Debug.Assert(true, "Operations have wrong types");
        return CreateNoneOp(this);
    }

    public static Operation CreateNoneOp(int userID = 0)
    {
        return new Operation(OperationType.None, -1, "", userID);
    }

    public static Operation CreateNoneOp(Operation oldOp)
    {
        return new Operation(OperationType.None, -1, "", oldOp.UserID);
    }

    public static Operation CreateInsertOp(int index, string text, int userID = 0)
    {
        return new Operation(OperationType.Insert, index, text, userID);
    }

    public static Operation CreateInsertOp(int index, string text, Operation oldOp)
    {
        return new Operation(OperationType.Insert, index, text, oldOp.UserID);
    }

    public static Operation CreateDeleteOp(int index, string text, int userID = 0)
    {
        return new Operation(OperationType.Delete, index, text, userID);
    }

    public static Operation CreateDeleteOp(int index, string text, Operation oldOp)
    {
        return new Operation(OperationType.Delete, index, text, oldOp.UserID);
    }

    public static Operation TransformInsertInsert(Operation op1, Operation op2, bool op1Priority = false)
    {
        Debug.Assert(op1.Type == OperationType.Insert && op2.Type == OperationType.Insert);

        if (op1.Pos < op2.Pos || (op1.Pos == op2.Pos && op1Priority))
        {
            return op1;
        }

        return CreateInsertOp(op1.Pos + op2.Text.Length, op1.Text, op1);
    }

    public static Operation TransformInsertDelete(Operation op1, Operation op2)
    {
        Debug.Assert(op1.Type == OperationType.Insert && op2.Type == OperationType.Delete);

        if (op1.Pos <= op2.Pos)
        {
            return op1;
        }

        if (op1.Pos > op2.Pos + op2.Text.Length - 1)
        {
            return CreateInsertOp(op1.Pos - op2.Text.Length, op1.Text, op1);
        }

        return CreateNoneOp(op1);
    }

    public static Operation TransformDeleteInsert(Operation op1, Operation op2)
    {
        Debug.Assert(op1.Type == OperationType.Delete && op2.Type == OperationType.Insert);

        if (op1.Pos + op1.Text.Length - 1 < op2.Pos)
        {
            return op1;
        }

        if (op1.Pos >= op2.Pos)
        {
            return CreateDeleteOp(op1.Pos + op2.Text.Length, op1.Text, op1);
        }

        //Combine delete text and insert text 
        var prefix = op1.Text.AsSpan(0, op2.Pos - op1.Pos);
        var postfix = op1.Text.AsSpan(op2.Pos - op1.Pos, (op1.Pos + op1.Text.Length + op2.Text.Length - 1) - (op2.Pos + op2.Text.Length - 1));
        var str = new StringBuilder().Append(prefix).Append(op2.Text).Append(postfix).ToString();

        return CreateDeleteOp(op1.Pos, str, op1);
    }

    public static Operation TransformDeleteDelete(Operation op1, Operation op2)
    {
        Debug.Assert(op1.Type == OperationType.Delete && op2.Type == OperationType.Delete);

        if (op1.Pos == op2.Pos && op1.Text.Length == op2.Text.Length)
        {
            return CreateNoneOp(op1);
        }

        //If op1 is to the left or to the right of op2 just adjust op1.pos if needed
        if (op1.Pos + op1.Text.Length - 1 < op2.Pos)
        {
            return op1;
        }
        if (op1.Pos > op2.Pos + op2.Text.Length - 1)
        {
            return CreateDeleteOp(op1.Pos - op2.Text.Length, op1.Text, op1);
        }

        //If op2 is before or at the same pos as op1 delete only what was not deleted by op2
        if (op2.Pos <= op1.Pos)
        {
            if (op1.Pos + op1.Text.Length <= op2.Pos + op2.Text.Length)
            {
                return CreateNoneOp(op1);
            }

            return CreateDeleteOp(op2.Pos, op1.Text.Substring(op2.Pos + op2.Text.Length - op1.Pos), op1);
        }


        //If op2 is after op1 delete only what was not deleted by op2
        if (op2.Pos + op2.Text.Length < op1.Pos + op1.Text.Length)
        {
            var prefix = op1.Text.AsSpan(0, op2.Pos - op1.Pos);
            var postfix = op1.Text.AsSpan(op2.Pos - op1.Pos + op2.Text.Length, op1.Text.Length - prefix.Length - op2.Text.Length);
            var str = new StringBuilder().Append(prefix).Append(postfix).ToString();
            return CreateDeleteOp(op1.Pos, str, op1);
        }

        return CreateDeleteOp(op1.Pos, op1.Text.Substring(0, op2.Pos - op1.Pos), op1);
    }
}