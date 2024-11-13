import { Operation, OperationType } from '../models/operation';

export const transformOperation = (incomingOp: Operation, localOps: Operation[]): Operation => {

    const transformedOp = { ...incomingOp };

    for (const localOp of localOps) {
        if (localOp.type === OperationType.Insert) {
            if (transformedOp.type === OperationType.Insert) {
                if (localOp.pos <= transformedOp.pos) {
                    transformedOp.pos += localOp.text.length;
                }
            } else if (transformedOp.type === OperationType.Delete) {
                if (localOp.pos <= transformedOp.pos) {
                    transformedOp.pos += localOp.text.length;
                } else if (localOp.pos < transformedOp.pos + transformedOp.text.length) {
                    transformedOp.text = transformedOp.text.slice(0, localOp.pos - transformedOp.pos) + transformedOp.text.slice(localOp.pos - transformedOp.pos + localOp.text.length);
                }
            }
        } else if (localOp.type === OperationType.Delete) {
            if (transformedOp.type === OperationType.Insert) {
                if (localOp.pos < transformedOp.pos) {
                    transformedOp.pos -= localOp.text.length;
                } else if (localOp.pos < transformedOp.pos + transformedOp.text.length) {
                    transformedOp.text = transformedOp.text.slice(0, localOp.pos - transformedOp.pos) + transformedOp.text.slice(localOp.pos - transformedOp.pos + localOp.text.length);
                }
            } else if (transformedOp.type === OperationType.Delete) {
                if (localOp.pos + localOp.text.length <= transformedOp.pos) {
                    transformedOp.pos -= localOp.text.length;
                } else if (localOp.pos < transformedOp.pos + transformedOp.text.length) {
                    transformedOp.text = transformedOp.text.slice(0, localOp.pos - transformedOp.pos) + transformedOp.text.slice(localOp.pos - transformedOp.pos + localOp.text.length);
                }
            }
        }
    }

    return transformedOp;
};
