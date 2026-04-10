import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Product } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private readonly apiUrl = 'https://localhost:5003/api/products';

  constructor(private http: HttpClient) { }

  /**
   * Get all products
   */
  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  /**
   * Get product by id
   */
  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create product
   */
  create(product: Product): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, product);
  }

  /**
   * Update product
   */
  update(id: string, product: Product): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, product);
  }

  /**
   * Delete product
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Decrease stock
   */
  baixar(id: string, quantidade: number): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/${id}/baixar?quantidade=${quantidade}`, 
      {}
    );
  }
}