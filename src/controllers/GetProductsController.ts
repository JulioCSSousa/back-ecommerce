import { Request, Response } from 'express';
import { AppDataSource } from '../database/data-source';
import { Product } from '../entity/Product';
import { Like } from 'typeorm';
import { toProductDto } from '../transformers/toProductDto'

export class GetProductsController {


    public async getProducts(req: Request, res: Response): Promise<Response> {
        const productRepository = AppDataSource.getRepository(Product);

        const page = parseInt(req.query.page as string, 10) || 1;
        const limit = parseInt(req.query.limit as string, 10) || 10;
        const offset = (page - 1) * limit;
        let [product, total]: any = [productRepository];

        const name = req.query.name;

        if(name){
            [product, total] = await productRepository.findAndCount({
                skip: offset,
                take: limit,
                where: {name: Like(`%${name}%`)}
            });
        } else{
            [product, total] = await productRepository.findAndCount({
                skip: offset,
                take: limit
            });
        }

        const totalPages = Math.ceil(total / limit);

        const result = toProductDto(product)
        console.log(result)
        return res.json({
            data: result,

            meta: {
                total,
                page,
                limit,
                totalPages,
                name
            }
        });
    }

}
