export class AuthModel {
  uuid: string;
  userId: string;
  username: string;
  Fullname: string;
  Avatar: string;
  Birthday: string;
  Phonenumber: string;
  Jobtitle: string;
  Departmemt: string;
  Email: string;
  setAuth(auth: AuthModel) {
    this.uuid = auth.uuid;
    this.userId = auth.userId;
    this.username = auth.username;
    this.Fullname = auth.Fullname;
    this.Avatar = auth.Avatar;
    this.Birthday = auth.Birthday;
    this.Phonenumber = auth.Phonenumber;
    this.Jobtitle = auth.Jobtitle;
    this.Departmemt = auth.Departmemt;
    this.Email = auth.Email;
  }
}
