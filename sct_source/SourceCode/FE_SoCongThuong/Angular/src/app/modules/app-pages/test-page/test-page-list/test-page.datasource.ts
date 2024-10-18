import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { QueryParamsModel, QueryResultsModel } from 'src/app/modules/auth/models/query-params.model';
import { BaseDataSource } from 'src/app/modules/auth/models/_base.datasource';
import { TestPageService } from '../test-page.service';

export class TestPageDataSource extends BaseDataSource {
	constructor(private _Service: TestPageService) {
		super();
	}

	LoadList(queryParams: QueryParamsModel) {
		this._Service.lastFilter$.next(queryParams);
        this.loadingSubject.next(true);
		this._Service.getData(queryParams)
			.pipe(
				tap(res => {
					this.entitySubject.next(res.data);
					this.paginatorTotalSubject.next(res.page.total);
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe();
	}
}
