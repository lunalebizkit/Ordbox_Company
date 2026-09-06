import { RolModel } from "./rol.model";

export interface UserModel {
  id:number;
  firstName: string;
  lastName: string;
  userName: string;
  password: string | null;
  email: string;
  roleId: number;
}
