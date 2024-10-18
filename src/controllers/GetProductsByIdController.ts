import { Request, Response } from 'express';
import { AppDataSource } from '../database/data-source';
import { Product } from '../entity/Product';

export class GetProductsByIdController {

    public async getProductById(req: Request, res: Response): Promise<Response> {
        const productRepository = AppDataSource.getRepository(Product);
        
        const id = req.params.id

        const product = await productRepository.findOne({
            where: { id: id },
        });

        if (!product){
            return res.status(404).json({ message: 'Product not found' });
        }
        
        return res.json(product);
        


        
    }

}
