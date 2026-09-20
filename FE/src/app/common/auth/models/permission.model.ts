import { Permission } from './permissions.enum';

export class PermissionModel {
  url!: RegExp;
  permissions!: Permission[];
}
