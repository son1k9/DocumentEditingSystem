export enum OperationType {
    None = 0,
    Insert = 1,
    Delete = 2
}

export class Operation {
    public type: OperationType;
    public pos: number;
    public text: string;
    public userID: number;

    constructor(type: OperationType, pos: number, text: string, userID: number) {
        this.type = type;
        this.pos = pos;
        this.text = text;
        this.userID = userID;
    }

    public transform(op: Operation, thisOpWins = false): Operation {
        if (this.type === OperationType.None || op.type === OperationType.None) {
            return this;
        }

        if (this.type === OperationType.Insert) {
            if (op.type === OperationType.Insert) {
                return Operation.transformInsertInsert(this, op, thisOpWins);
            }
            if (op.type === OperationType.Delete) {
                return Operation.transformInsertDelete(this, op);
            }
        }

        if (this.type === OperationType.Delete) {
            if (op.type === OperationType.Insert) {
                return Operation.transformDeleteInsert(this, op);
            }
            if (op.type === OperationType.Delete) {
                return Operation.transformDeleteDelete(this, op);
            }
        }

        console.assert(false, "Operations have wrong types");
        return Operation.createNoneOp(-1);
    }

    public static createNoneOp(userID = 0): Operation {
        return new Operation(OperationType.None, -1, "", userID);
    }

    public static createNoneOpFromOldOp(oldOp: Operation): Operation {
        return new Operation(OperationType.None, -1, "", oldOp.userID);
    }

    public static createInsertOp(index: number, text: string, userID = 0): Operation {
        return new Operation(OperationType.Insert, index, text, userID);
    }

    public static createInsertOpFromOldOp(index: number, text: string, oldOp: Operation): Operation {
        return new Operation(OperationType.Insert, index, text, oldOp.userID);
    }

    public static createDeleteOp(index: number, text: string, userID = 0): Operation {
        return new Operation(OperationType.Delete, index, text, userID);
    }

    public static createDeleteOpFromOldOp(index: number, text: string, oldOp: Operation): Operation {
        return new Operation(OperationType.Delete, index, text, oldOp.userID);
    }

    public static transformInsertInsert(op1: Operation, op2: Operation, op1Priority = false): Operation {
        console.assert(op1.type === OperationType.Insert && op2.type === OperationType.Insert);

        if (op1.pos < op2.pos || (op1.pos === op2.pos && op1Priority)) {
            return op1;
        }

        return Operation.createInsertOp(op1.pos + op2.text.length, op1.text, op1.userID);
    }

    public static transformInsertDelete(op1: Operation, op2: Operation): Operation {
        console.assert(op1.type === OperationType.Insert && op2.type === OperationType.Delete);

        if (op1.pos <= op2.pos) {
            return op1;
        }

        if (op1.pos > op2.pos + op2.text.length - 1) {
            return Operation.createInsertOp(op1.pos - op2.text.length, op1.text, op1.userID);
        }

        return Operation.createNoneOpFromOldOp(op1);
    }

    public static transformDeleteInsert(op1: Operation, op2: Operation): Operation {
        console.assert(op1.type === OperationType.Delete && op2.type === OperationType.Insert);

        if (op1.pos + op1.text.length - 1 < op2.pos) {
            return op1;
        }

        if (op1.pos >= op2.pos) {
            return Operation.createDeleteOp(op1.pos + op2.text.length, op1.text, op1.userID);
        }

        const prefix = op1.text.slice(0, op2.pos - op1.pos);
        const postfix = op1.text.slice(op2.pos - op1.pos + op2.text.length);
        const str = prefix + op2.text + postfix;

        return Operation.createDeleteOp(op1.pos, str, op1.userID);
    }

    public static transformDeleteDelete(op1: Operation, op2: Operation): Operation {
        console.assert(op1.type === OperationType.Delete && op2.type === OperationType.Delete);

        if (op1.pos === op2.pos && op1.text.length === op2.text.length) {
            return Operation.createNoneOpFromOldOp(op1);
        }

        if (op1.pos + op1.text.length - 1 < op2.pos) {
            return op1;
        }
        if (op1.pos > op2.pos + op2.text.length - 1) {
            return Operation.createDeleteOp(op1.pos - op2.text.length, op1.text, op1.userID);
        }

        if (op2.pos <= op1.pos) {
            if (op1.pos + op1.text.length <= op2.pos + op2.text.length) {
                return Operation.createNoneOpFromOldOp(op1);
            }

            return Operation.createDeleteOp(op2.pos, op1.text.substring(op2.pos + op2.text.length - op1.pos), op1.userID);
        }

        if (op2.pos + op2.text.length < op1.pos + op1.text.length) {
            const prefix = op1.text.slice(0, op2.pos - op1.pos);
            const postfix = op1.text.slice(op2.pos - op1.pos + op2.text.length);
            const str = prefix + postfix;
            return Operation.createDeleteOp(op1.pos, str, op1.userID);
        }

        return Operation.createDeleteOp(op1.pos, op1.text.substring(0, op2.pos - op1.pos), op1.userID);
    }
}
