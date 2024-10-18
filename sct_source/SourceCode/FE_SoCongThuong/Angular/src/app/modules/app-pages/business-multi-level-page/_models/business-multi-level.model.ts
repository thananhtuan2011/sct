import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface BusinessMultiLevelModel extends BaseModel {
    businessMultiLevelId: string;
    businessId: string;
    districtId: string;
    address: string;
    startDate: any;
    status: string;
    numCert: string;
    certDate: any;
    certExp: any;
    contact: string;
    phoneNumber: string;
    addressContact: string;
    goods: string;
    localConfirm: string;
    note: string;
}
