export enum OperationType {
    None = 0,
    Insert = 1,
    Delete = 2
}

export class Operation {
    public Type: OperationType;
    public Pos: number;
    public Text: string;
    public UserID: number;

    constructor(type: OperationType, pos: number, text: string, userID: number) {
        this.Type = type;
        this.Pos = pos;
        this.Text = text;
        this.UserID = userID;
    }

    public transform(op: Operation, thisOpWins = false): Operation {
        if (this.Type === OperationType.None || op.Type === OperationType.None) {
            return this;
        }

        if (this.Type === OperationType.Insert) {
            if (op.Type === OperationType.Insert) {
                return Operation.transformInsertInsert(this, op, thisOpWins);
            }
            if (op.Type === OperationType.Delete) {
                return Operation.transformInsertDelete(this, op);
            }
        }

        if (this.Type === OperationType.Delete) {
            if (op.Type === OperationType.Insert) {
                return Operation.transformDeleteInsert(this, op);
            }
            if (op.Type === OperationType.Delete) {
                return Operation.transformDeleteDelete(this, op);
            }
        }

        console.assert(false, "Operations have wrong types");
        return Operation.createNoneOp(this);
    }

    public static createNoneOp(userID = 0): Operation {
        return new Operation(OperationType.None, -1, "", userID);
    }

    public static createNoneOpFromOldOp(oldOp: Operation): Operation {
        return new Operation(OperationType.None, -1, "", oldOp.UserID);
    }

    public static createInsertOp(index: number, text: string, userID = 0): Operation {
        return new Operation(OperationType.Insert, index, text, userID);
    }

    public static createInsertOpFromOldOp(index: number, text: string, oldOp: Operation): Operation {
        return new Operation(OperationType.Insert, index, text, oldOp.UserID);
    }

    public static createDeleteOp(index: number, text: string, userID = 0): Operation {
        return new Operation(OperationType.Delete, index, text, userID);
    }

    public static createDeleteOpFromOldOp(index: number, text: string, oldOp: Operation): Operation {
        return new Operation(OperationType.Delete, index, text, oldOp.UserID);
    }

    public static transformInsertInsert(op1: Operation, op2: Operation, op1Priority = false): Operation {
        console.assert(op1.Type === OperationType.Insert && op2.Type === OperationType.Insert);

        if (op1.Pos < op2.Pos || (op1.Pos === op2.Pos && op1Priority)) {
            return op1;
        }

        return Operation.createInsertOp(op1.Pos + op2.Text.length, op1.Text, op1.UserID);
    }

    public static transformInsertDelete(op1: Operation, op2: Operation): Operation {
        console.assert(op1.Type === OperationType.Insert && op2.Type === OperationType.Delete);

        if (op1.Pos <= op2.Pos) {
            return op1;
        }

        if (op1.Pos > op2.Pos + op2.Text.length - 1) {
            return Operation.createInsertOp(op1.Pos - op2.Text.length, op1.Text, op1.UserID);
        }

        return Operation.createNoneOpFromOldOp(op1);
    }

    public static transformDeleteInsert(op1: Operation, op2: Operation): Operation {
        console.assert(op1.Type === OperationType.Delete && op2.Type === OperationType.Insert);

        if (op1.Pos + op1.Text.length - 1 < op2.Pos) {
            return op1;
        }

        if (op1.Pos >= op2.Pos) {
            return Operation.createDeleteOp(op1.Pos + op2.Text.length, op1.Text, op1.UserID);
        }

        const prefix = op1.Text.slice(0, op2.Pos - op1.Pos);
        const postfix = op1.Text.slice(op2.Pos - op1.Pos + op2.Text.length);
        const str = prefix + op2.Text + postfix;

        return Operation.createDeleteOp(op1.Pos, str, op1.UserID);
    }

    public static transformDeleteDelete(op1: Operation, op2: Operation): Operation {
        console.assert(op1.Type === OperationType.Delete && op2.Type === OperationType.Delete);

        if (op1.Pos === op2.Pos && op1.Text.length === op2.Text.length) {
            return Operation.createNoneOpFromOldOp(op1);
        }

        if (op1.Pos + op1.Text.length - 1 < op2.Pos) {
            return op1;
        }
        if (op1.Pos > op2.Pos + op2.Text.length - 1) {
            return Operation.createDeleteOp(op1.Pos - op2.Text.length, op1.Text, op1.UserID);
        }

        if (op2.Pos <= op1.Pos) {
            if (op1.Pos + op1.Text.length <= op2.Pos + op2.Text.length) {
                return Operation.createNoneOpFromOldOp(op1);
            }

            return Operation.createDeleteOp(op2.Pos, op1.Text.substring(op2.Pos + op2.Text.length - op1.Pos), op1.UserID);
        }

        if (op2.Pos + op2.Text.length < op1.Pos + op1.Text.length) {
            const prefix = op1.Text.slice(0, op2.Pos - op1.Pos);
            const postfix = op1.Text.slice(op2.Pos - op1.Pos + op2.Text.length);
            const str = prefix + postfix;
            return Operation.createDeleteOp(op1.Pos, str, op1.UserID);
        }

        return Operation.createDeleteOp(op1.Pos, op1.Text.substring(0, op2.Pos - op1.Pos), op1.UserID);
    }
}
