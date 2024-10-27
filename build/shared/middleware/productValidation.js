"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.productValidation = exports.productSchema = void 0;
const http_status_codes_1 = require("http-status-codes");
const yup = __importStar(require("yup"));
exports.productSchema = yup.object().shape({
    name: yup.string().required("Entre com um nome"),
    description: yup.string().optional(),
    price: yup.number().required("O preço é obrigatório")
});
async function productValidation(request, response, next) {
    try {
        await exports.productSchema.validate(request.body, { abortEarly: false });
        next();
    }
    catch (exceptions) {
        const validationErrors = {};
        const yupError = exceptions;
        yupError.inner.forEach((error) => {
            validationErrors[error.path] = error.message;
        });
        return response.status(http_status_codes_1.StatusCodes.BAD_REQUEST).json({
            validationErrors
        });
    }
}
exports.productValidation = productValidation;
//# sourceMappingURL=productValidation.js.map