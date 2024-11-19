namespace OperationalTransformation.Test;

public static class StringExtension
{
    public static string ApplyOp(this string str, Operation op)
    {
        if (op.Type == OperationType.Insert)
        {
            return str.Insert(op.Pos, op.Text);
        }

        if (op.Type == OperationType.Delete)
        {
            return str.Remove(op.Pos, op.Text.Length);
        }

        return str;
    }
}