import { ProductNoIDResponse } from '../responses/productDto';
import { Product } from '../entity/Product';
import { ProductDto } from '../dtos/productDto';

export function toProductDto(product: Product[]): ProductDto[] {
    return product.map((product) => {
        return new ProductDto({
            id: product.id,
            name: product.name,
            description: product.description,
            image: product.image,
            price: product.price
        });
    });
};
