export interface Node {
    id: string;
    name: string;
    role: string;
    children?: Node[];
    parent?: Node;
}