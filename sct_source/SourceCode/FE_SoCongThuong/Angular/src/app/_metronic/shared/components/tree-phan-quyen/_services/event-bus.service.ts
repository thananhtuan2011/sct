import { Subject } from 'rxjs';

export class EventBusService {
	nodeCheckedChange = new Subject();
	pageRoutingChange = new Subject();
}
