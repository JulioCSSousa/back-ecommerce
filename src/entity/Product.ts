import { Entity, PrimaryGeneratedColumn, Column} from 'typeorm';

@Entity()
export class Product {
  @PrimaryGeneratedColumn('uuid')
  id?: string;

  @Column({ length: 100 })
  name?: string;

  @Column({ length: 500, nullable: true })
  description?: string | null;


  @Column({ length: 300, nullable: true })
  image?: string | null;

  @Column({ nullable: true })
  price?: number;

}

