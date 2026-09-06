interface WindowEnv {
  env?: {
    apiUrl?: string;
    apiVer?: number;
  };
}

declare const window: WindowEnv;

export const environment = {
  production: false,
  sistemaGestion:"http://apigestion.kiltex.com.ar/",
  api:{
    url: window?.env?.apiUrl ?? 'http://apigestion.local/api/',
    ver: window?.env?.apiVer ?? 1
  },
  name: "Refrigeraciones Dante",
};