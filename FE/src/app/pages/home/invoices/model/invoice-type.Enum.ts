export const InvoiceType = [{ value: 1, label: 'A' },
{ value: 2, label: 'B' }];

export enum eInvoiceType {
  A = 1,
  B = 2,
  C = 3,
  EXENTO = 4,
  RespMonotributo = 6
}

export const IvaCondition = [{value: 1, label: 'Resp. Inscripto', disabled: false},
   {value: 2, label: 'Resp. Monotributo', disabled: false},
   {value: 3, label: 'Consumidor final', disabled: true},
    {value: 4, label: 'Exento', disabled: true}];

export enum eIvaCondition {
  RespInscrip = 1,
  RespMonotributo = 2,
  ConsFinal = 3,
  Exento = 4,
}