import { Permission } from "./permissions.enum";

export class AuthUserModel {
  userName!: string;
  fullName!: string;
  refreshToken!: string;
  token!: string;
  rol!: string;
  permission!: Permission[];
  id!:number;
  expiration!: string;
}
