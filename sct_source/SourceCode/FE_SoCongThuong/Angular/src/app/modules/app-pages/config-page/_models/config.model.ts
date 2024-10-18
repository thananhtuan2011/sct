import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface GroupConfigModel extends BaseModel {
    categoryTypeId: string,
    categoryTypeCode: string,
    categoryTypeName: string,
    description: string,
}

export class GroupConfigDefault {
    id: "";
    categoryTypeId: "00000000-0000-0000-0000-000000000000";
    categoryTypeCode: "";
    categoryTypeName: "";
    description: "";
}

export interface ConfigModel {
    categoryId: string,
    categoryTypeCode: string,
    categoryCode: string,
    categoryName: string,
    priority: number,
    isAction: boolean,
    isDel: boolean
}

export interface ListConfigModel extends BaseModel {
    ListConfig: ConfigModel[]
}

export class ConfigDefault {
    Config: ConfigModel = {
        categoryId: "00000000-0000-0000-0000-000000000000",
        categoryTypeCode: "",
        categoryCode: "",
        categoryName: "",
        priority: 0,
        isAction: true,
        isDel: false
    };
}