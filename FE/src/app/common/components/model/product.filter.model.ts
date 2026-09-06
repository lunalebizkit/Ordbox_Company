export interface ProductFilter{
filter: {
      product: string | null,
      brand: number,
      category: number,
      status: number,
      date: Date | null,
      supplier: [number] | [],

    },
    page: number,
    pageSize: number,
};

export const resetProductFilter: ProductFilter = {
  filter: {
    product: null,
    category: 0,
    brand: 0,
    status: 0,
    date: null,
    supplier: []
  },
  page: 0,
  pageSize: 20
};