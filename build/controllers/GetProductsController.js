"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetProductsController = void 0;
const data_source_1 = require("../database/data-source");
const Product_1 = require("../entity/Product");
const typeorm_1 = require("typeorm");
const toProductDto_1 = require("../transformers/toProductDto");
class GetProductsController {
    async getProducts(req, res) {
        const productRepository = data_source_1.AppDataSource.getRepository(Product_1.Product);
        const page = parseInt(req.query.page, 10) || 1;
        const limit = parseInt(req.query.limit, 10) || 10;
        const offset = (page - 1) * limit;
        let [product, total] = [productRepository];
        const name = req.query.name;
        if (name) {
            [product, total] = await productRepository.findAndCount({
                skip: offset,
                take: limit,
                where: { name: (0, typeorm_1.Like)(`%${name}%`) }
            });
        }
        else {
            [product, total] = await productRepository.findAndCount({
                skip: offset,
                take: limit
            });
        }
        const totalPages = Math.ceil(total / limit);
        const result = (0, toProductDto_1.toProductDto)(product);
        console.log(result);
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
exports.GetProductsController = GetProductsController;
//# sourceMappingURL=GetProductsController.js.map