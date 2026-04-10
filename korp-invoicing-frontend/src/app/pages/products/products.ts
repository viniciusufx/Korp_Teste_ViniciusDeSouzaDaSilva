import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ProductService } from '../../services/product';
import { Product } from '../../models/product';

import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.html',
  styleUrl: './products.scss'
})
export class ProductsComponent implements OnInit {

  products: Product[] = [];

  newProduct: Product = {
    id: '',
    codigo: '',
    descricao: '',
    saldo: 0
  };

  loading = false;

  editingProductId: string | null = null;

  constructor(
    private productService: ProductService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts() {
    this.loading = true;

    this.productService.getAll().subscribe({
      next: (data) => {
        this.products = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.loading = false;

        this.extractErrorMessages(err)
          .forEach(msg => this.toastr.error(msg, 'Erro'));

        this.cdr.detectChanges();
      }
    });
  }

  createProduct() {
    this.productService.create(this.newProduct).subscribe({
      next: () => {
        this.toastr.success('Produto criado com sucesso!', 'Sucesso');

        this.resetForm();
        this.loadProducts();
      },
      error: (err) => {
        this.extractErrorMessages(err)
          .forEach(msg => this.toastr.error(msg, 'Erro ao criar produto'));
      }
    });
  }

  startEdit(product: Product) {
    this.editingProductId = product.id;

    this.newProduct = {
      id: product.id,
      codigo: product.codigo,
      descricao: product.descricao,
      saldo: product.saldo
    };
  }

  updateProduct() {
    if (!this.editingProductId) return;

    this.productService.update(this.editingProductId, this.newProduct).subscribe({
      next: () => {
        this.toastr.success('Produto atualizado com sucesso!', 'Sucesso');

        this.resetForm();
        this.loadProducts();
      },
      error: (err) => {
        this.extractErrorMessages(err)
          .forEach(msg => this.toastr.error(msg, 'Erro ao atualizar'));
      }
    });
  }

  cancelEdit() {
    this.resetForm();
  }

  deleteProduct(id: string) {
    if (!confirm('Deseja remover o produto?')) return;

    this.productService.delete(id).subscribe({
      next: () => {
        this.toastr.success('Produto removido com sucesso!');
        this.loadProducts();
      },
      error: (err) => {
        this.extractErrorMessages(err)
          .forEach(msg => this.toastr.error(msg, 'Erro ao remover'));
      }
    });
  }

  resetForm() {
    this.editingProductId = null;

    this.newProduct = {
      id: '',
      codigo: '',
      descricao: '',
      saldo: 0
    };
  }

  private extractErrorMessages(error: any): string[] {
    if (!error) return ['Erro inesperado.'];

    if (error.error?.errors) {
      const errors = error.error.errors;
      let messages: string[] = [];

      Object.keys(errors).forEach(field => {
        errors[field].forEach((msg: string) => {
          messages.push(`${field}: ${msg}`);
        });
      });

      return messages;
    }

    if (error.error?.message) return [error.error.message];
    if (error.error?.title) return [error.error.title];
    if (typeof error.error === 'string') return [error.error];

    return ['Erro ao processar requisição.'];
  }
}