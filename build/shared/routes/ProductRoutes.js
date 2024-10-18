"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = require("express");
const index_1 = require("../../controllers/index");
const productValidation_1 = require("../middleware/productValidation");
const router = (0, express_1.Router)();
const getProduct = new index_1.GetProductsController();
const getProductById = new index_1.GetProductsByIdController();
const createProduct = new index_1.CreateProductsController();
const patchProduct = new index_1.PatchProductsController();
const deleteProduct = new index_1.DeleteProductsController();
router.post('/api/products', productValidation_1.productValidation, createProduct.createProduct.bind(createProduct));
router.get('/api/products', getProduct.getProducts.bind(getProduct));
router.get('/api/products/:id', getProductById.getProductById.bind(getProductById));
router.patch('/api/products/:id', patchProduct.patchProduct.bind(patchProduct));
router.delete('/api/products/:id', deleteProduct.deleteProduct.bind(deleteProduct));
exports.default = router;
//# sourceMappingURL=ProductRoutes.js.map