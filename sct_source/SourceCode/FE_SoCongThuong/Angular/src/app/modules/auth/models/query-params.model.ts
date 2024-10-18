export interface IEdit {
	_isEditMode: boolean;
	_isNew: boolean;
	_isDeleted: boolean;
	_isUpdated: boolean;
	_prevState: any;
}
export interface IFilter {
	_defaultFieldName: string; // Field which should filtered first
}
export interface ILog {
	_userId: number; // user who did changes
	_createdDate: string; // date when entity were created => format: 'mm/dd/yyyy'
	_updatedDate: string; // date when changed were applied => format: 'mm/dd/yyyy'
}

export class BaseModel implements IEdit, IFilter, ILog {
	// Edit
	_isEditMode: boolean = false;
	_isNew: boolean = false;
	_isUpdated: boolean = false;
	_isDeleted: boolean = false;
	_prevState: any = null;
	// Filter
	_defaultFieldName: string = '';
	// Log
	_userId: number = 0; // Admin
	_createdDate: string;
	_updatedDate: string;
}

export class QueryParamsModel {
	// fields
	filter: any;
	sortOrder: string; // asc || desc
	sortField: string;
	pageNumber: number;
	pageSize: number;

	// constructor overrides
	constructor(_filter: any,
		_sortOrder: string = 'asc',
		_sortField: string = '',
		_pageNumber: number = 0,
		_pageSize: number = 5) {
		this.filter = _filter;
		this.sortOrder = _sortOrder;
		this.sortField = _sortField;
		this.pageNumber = _pageNumber;
		this.pageSize = _pageSize;
	}
}
export class QueryResultsModel {
	// fields
	data: any[];
	items: any[];
	totalCount: number;
	errorMessage: string;
	page: any;
	error: any;

	constructor(_items: any[] = [], _totalCount: number = 0, _errorMessage: string = '') {
		this.items = this.data = _items;
		this.totalCount = _totalCount;
	}
}