export interface PermissionRol{
    id: number,
    rol: string,
    permissions: PermissionModel[]
};
export interface PermissionModel{
    id: number,
    name: string,
    key: string,
    enumPermission: number
}
export interface AddOrUpdatePermission {
    id: number,
    name: '',
    key: '',
    permissionIds: number[]
}