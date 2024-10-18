import { BaseModel } from "src/app/modules/auth/models/query-params.model"


export class DemoModel extends BaseModel {

    id: number = 0

    name : string = ''

    clear() {

        this.id = 0;

        this.name = '';

    }
}
