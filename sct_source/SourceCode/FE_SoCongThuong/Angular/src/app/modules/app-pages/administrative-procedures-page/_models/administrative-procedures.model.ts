import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface AdministrativeProceduresModel extends BaseModel {
    administrativeProceduresId: string,
    administrativeProceduresField: string,
    administrativeProceduresCode: string,
    status: any,
    receptionForm: any,
    administrativeProceduresName: string,
    amountOfRecords: any,
    dayReception: any,
    settlementTerm: any,
    finishDay: any,
}
