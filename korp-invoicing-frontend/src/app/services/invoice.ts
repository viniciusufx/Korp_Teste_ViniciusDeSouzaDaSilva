import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice } from '../models/invoice';

@Injectable({ providedIn: 'root' })
export class InvoiceService {

  private readonly api = 'https://localhost:5001/api/Invoices';

  constructor(private http: HttpClient) {}

  getAll(page = 1, size = 15): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(`${this.api}?pageNumber=${page}&pageSize=${size}`);
  }

  create(payload: { items: any[] }): Observable<any> {
    return this.http.post(this.api, payload);
  }

  print(id: string): Observable<any> {
    return this.http.post(`${this.api}/${id}/print`, {});
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.api}/${id}`);
  }
}