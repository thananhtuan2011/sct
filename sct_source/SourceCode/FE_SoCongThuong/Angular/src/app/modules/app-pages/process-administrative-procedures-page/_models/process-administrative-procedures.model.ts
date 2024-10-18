import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ProcessAdministrativeProceduresModel extends BaseModel {
    processAdministrativeProceduresId: string,
    processAdministrativeProceduresField: string,
    processAdministrativeProceduresCode: string,
    processAdministrativeProceduresName: string,
    processStep: any,
}
