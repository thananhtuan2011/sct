import { AuthModel } from './auth.model';
import { AddressModel } from './address.model';
import { SocialNetworksModel } from './social-networks.model';

export class UserModel extends AuthModel {
  id: number;
  username: string;
  password: string;
  fullname: string;
  email: string;
  cccd: string;
  pic: string;
  status : number;
  roles: string;
  groupname: string;
  deptName: string;
  occupation: string;
  companyName: string;
  phonenumber: string;
  address?: AddressModel;
  groupId:string;
  deptId: string;
  roleId: string;
  socialNetworks?: SocialNetworksModel;
  // personal information
  firstname: string;
  lastname: string;
  website: string;
  // account information
  language: string;
  timeZone: string;
  avatar?: string;
  communication: {
    email: boolean;
    sms: boolean;
    phone: boolean;
    departmemt: string;
    departmemtName: string;
    departmemtID: number;
  };
  // email settings
  emailSettings?: {
    emailNotification: boolean;
    sendCopyToPersonalEmail: boolean;
    activityRelatesEmail: {
      youHaveNewNotifications: boolean;
      youAreSentADirectMessage: boolean;
      someoneAddsYouAsAsAConnection: boolean;
      uponNewOrder: boolean;
      newMembershipApproval: boolean;
      memberRegistration: boolean;
    };
    updatesFromKeenthemes: {
      newsAboutKeenthemesProductsAndFeatureUpdates: boolean;
      tipsOnGettingMoreOutOfKeen: boolean;
      thingsYouMissedSindeYouLastLoggedIntoKeen: boolean;
      newsAboutMetronicOnPartnerProductsAndOtherServices: boolean;
      tipsOnMetronicBusinessProducts: boolean;
    };
  };

  setUser(_user: unknown) {
    const user = _user as UserModel;
    this.id = user.id;
    this.username = user.username || '';
    this.password = user.password || '';
    this.fullname = user.fullname || '';
    this.email = user.email || '';
    this.pic = user.pic || './assets/media/avatars/blank.png';
    this.roles = user.roles || '';
    this.occupation = user.occupation || '';
    this.companyName = user.companyName || '';
    this.phonenumber = user.phonenumber || '';
    this.address = user.address;
    this.socialNetworks = user.socialNetworks;
    this.avatar = user.avatar;
    this.groupname = user.groupname;
    this.deptName = user.deptName;
    this.cccd = user.cccd;
     this.status = user.status;
     this.groupId = user.groupId;
     this.deptId =user.deptId;
     this.roleId = user.roleId;
    // this.departmemt = user.departmemt;
    // this.departmemtName = user.departmemtName;
    // this.departmemtID= user.departmemtID;
  }
}

export class UserLoginFailModel {
  countLoginFail: number;
  error: string;
  timeLock: Date;
}
