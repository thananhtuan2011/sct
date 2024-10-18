import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface NewRuralCriteriaModel extends BaseModel {
    newRuralCriteriaId: string,
    districtId: string,
    communeId: string,
    titleIdStr: string,
    target4: boolean,
    target7: boolean,
    target1708: boolean,
    note: string,
}
