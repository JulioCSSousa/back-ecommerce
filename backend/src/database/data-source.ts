import "reflect-metadata"
import { DataSource } from "typeorm"
import * as dotenv from "dotenv"
import { Product } from "../entity/Product"



dotenv.config();


export const AppDataSource = new DataSource({
    type: "mysql",
    host: process.env.LOCAL_HOST,
    port: 3306,
    username: process.env.LOCAL_USER,
    password: process.env.LOCAL_PASSWORD,
    database: process.env.LOCAL_DB,
    synchronize: true,
    logging: false,
    entities: [Product],
    migrations: [__dirname + '/../../typeorm-migrations/*.{ts,js}']
})


