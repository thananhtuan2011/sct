import { NestedTreeControl } from "@angular/cdk/tree";
import { Component, ViewChild, AfterViewInit, OnInit, Input } from "@angular/core";
import { MatTreeNestedDataSource, MatTree } from "@angular/material/tree";
import { Node } from './tree-datasource';
import { UserService } from '../../../_services/user.service';

// let TREE_DATA: Node[] = [
//     {
//         name: "Fruit",
//         children: [{ name: "Apple" }, { name: "Banana" }, { name: "Fruit loops" }],
//     },
//     {
//         name: "Vegetables",
//         children: [
//             {
//                 name: "Green",
//                 children: [{ name: "Broccoli" }, { name: "Brussel sprouts" }]
//             },
//             {
//                 name: "Orange",
//                 children: [{ name: "Pumpkins" }, { name: "Carrots" }]
//             }
//         ]
//     }
// ];

@Component({
    selector: "app-tree-user-component",
    templateUrl: "tree-user-modal.component.html",
    styleUrls: ["tree-user-modal.component.scss"]
})

export class TreeUserComponent implements OnInit {
    @Input() id: string;
    treeControl: NestedTreeControl<Node>;
    dataSource: MatTreeNestedDataSource<Node>;

    constructor(
        private UserService: UserService,
    ) { }

    ngOnInit(): void {
        //Tree Control
        this.treeControl = new NestedTreeControl<Node>(this.getChildren);
        
        //Data Source
        this.dataSource = new MatTreeNestedDataSource<Node>();
        
        this.loadDataTree();
    }

    getChildren = (node: Node) => {
        return node.children;
    };

    hasChild = (_: number, node: Node) => !!node.children && node.children.length > 0;

    setParent(data: Node, parent: Node | null) {
        data.parent = parent ?? {} as Node;
        if (data.children) {
            data.children.forEach(x => {
                this.setParent(x, data);
            });
        }
    }

    loadDataTree() {
        this.UserService.getTreeData().subscribe((res: any) => {
            //Set InputData
            this.dataSource.data = res.items

            //Set parent:
            for (var item of this.dataSource.data) {
                this.setParent(item, null);
            }

            //Expand Parent
            let node = this.findById(this.id);
            if (node) {
                this.expandParent(node)
            }
        })
    }

    findById(id: string): Node | null {
        const stack = [...this.dataSource.data];
        while (stack.length > 0) {
            const node = stack.pop() ?? {} as Node;
            if (node.id === id) {
                return node;
            }
            if (node.children) {
                stack.push(...node.children);
            }
        }
        return null;
    }

    getParentNode(node: Node): Node | null {
        if (node.parent != undefined || node.parent != null) {
            return node.parent
        }
        return null;
    }

    expandParent(node: Node) {
        if (node !== undefined && node !== null) {
            let parent = this.getParentNode(node);
            while (parent) {
                this.treeControl.expand(parent);
                parent = this.getParentNode(parent);
                if (parent) {
                    this.expandParent(parent);
                }
            }
        }
    }
}
