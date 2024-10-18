import { Component, OnInit, EventEmitter, Input, Output, ViewChild, ElementRef } from '@angular/core';

import { EventBusService } from '../_services/event-bus.service';
@Component({
	selector: 'app-tree-phan-quyen-node',
	templateUrl: 'tree-phan-quyen-node.component.html'
})

export class TreePhanQuyenNodeComponent implements OnInit {

	@Input() nameNode: string = "keytext";//default là text
	@Input() nameNodeNoKey: string ="text";
	@Input() propNameChild: string = "children";//tên node roof có chứa các con của node, default = children
	@Input() propNameCss: string = "anCss";//tên thuộc tính sẽ chứa các định dạng css cho node, default = anCss
	@Input() valuePhanQuyen: any = [];//a list roles whill be made tree authorize
	@Input() parentNode: any = [];//parent node, get it for checked node child will need this node

	//phải gọi public thì thằng cha mới có thể nhận dữ liệu
	constructor(public eventBusService: EventBusService) {

	}

	ngOnInit() {

		this.duyetTree(this.valuePhanQuyen, {});
	}

	//duyệt node thêm một số thuộc tính để tiện cho việc xử lý dữ liệu
	duyetTree(node: any, parent: any) {
		//
		node.forEach((item:any, index:any) => {

			let anCss = {
				collapse: item[this.propNameChild] ? item[this.propNameChild].length > 0 ? true : false : false,
				lastChild: item[this.propNameChild] ? item[this.propNameChild].length > 0 ? false : true : true,
				state: 0,//trạng thái luôn luôn mở node này, 0 -> open, -1 -> close
				checked: false,
				parentChk: ''
			};
			if (item[this.propNameCss] == undefined)
				item[this.propNameCss] = anCss;

			if (item[this.propNameChild]) {
				if (item[this.propNameChild].length > 0) {
					this.duyetTree(item[this.propNameChild], item);
				}
			}
		});
	}

	// => đang thao tác đóng - mở các nút
	collapseChanged(v: any, state: number) {
		v[this.propNameCss].state = state;
		v[this.propNameCss].collapse = state == 0;
	}

	// -> checked change các node
	checkedChanged(v: any, e: any) {
		v[this.propNameCss].checked = e.checked;
		this.eventBusService.nodeCheckedChange.next(v);
	}

	//-> checked for parent node
	checkedParentNode(currentNode: any, event: any) {
	}

	phanQuyenChanged(v: any) {
		//console.log("From child node");
	}

}
