
export class OrderedProductDto {
    id?: string;
    storeId?: string;
    name?: string;
    description?: string;
    observation?: string;
    price?: number;

    constructor(
        id?: string,
        storeId?: string,
        name?: string,
        description?: string,
        observation?: string,
        price?: number,

    ) {

        this.id = id;
        this.storeId = storeId;
        this.name = name;
        this.description = description;
        this.observation = observation;
        this.price = price;
    }

}