export enum OperationType {
    None = 'None',
    Insert = 'Insert',
    Delete = 'Delete'
}

export interface Operation {
    type: OperationType;
    pos: number;
    text: string;
    version: number;
    userID: number;
}