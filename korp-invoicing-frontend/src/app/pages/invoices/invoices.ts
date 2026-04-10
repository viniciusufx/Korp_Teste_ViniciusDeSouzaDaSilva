import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { InvoiceService } from '../../services/invoice';
import { InvoiceItem } from '../../models/invoice-item';

import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './invoices.html',
  styleUrl: './invoices.scss'
})
export class InvoicesComponent implements OnInit {

  invoices: any[] = [];
  loading = false;

  newItem: InvoiceItem = {
    productId: '',
    quantity: 1
  };

  items: InvoiceItem[] = [];

  // 🔥 múltiplas notas abertas
  expandedInvoices: Set<string> = new Set();

  constructor(
    private service: InvoiceService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.loading = true;

    this.service.getAll().subscribe({
      next: (data) => {
        this.invoices = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.toastr.error('Erro ao carregar notas');
        this.cdr.detectChanges();
      }
    });
  }

  addItem() {
    this.items.push({ ...this.newItem });

    this.newItem = {
      productId: '',
      quantity: 1
    };
  }

  removeItem(i: number) {
    this.items.splice(i, 1);
  }

  create() {
    this.service.create({ items: this.items }).subscribe({
      next: () => {
        this.toastr.success('Nota criada!');
        this.items = [];
        this.load();
      },
      error: (err) => {
        this.toastr.error(this.extractError(err));
      }
    });
  }

  print(id: string) {
    this.service.print(id).subscribe({
      next: () => {
        this.toastr.success('Nota impressa e estoque baixado!');
        this.load();
      },
      error: (err) => {
        this.toastr.error(this.extractError(err));
      }
    });
  }

  delete(id: string) {
    const confirmDelete = confirm('Tem certeza que deseja deletar esta nota fiscal?');
  
    if (!confirmDelete) return;
  
    this.service.delete(id).subscribe({
      next: () => {
        this.toastr.success('Removido');
        this.load();
      },
      error: (err) => {
        this.toastr.error(this.extractError(err));
      }
    });
  }

  // 🔥 toggle múltiplo
  toggleItems(id: string) {
    if (this.expandedInvoices.has(id)) {
      this.expandedInvoices.delete(id);
    } else {
      this.expandedInvoices.add(id);
    }
  }

  isExpanded(id: string): boolean {
    return this.expandedInvoices.has(id);
  }

  getStatus(s: number) {
    return s === 0 ? 'Open' : 'Closed';
  }

  private extractError(error: any): string {
    if (error.error?.title) return error.error.title;

    if (error.error?.errors) {
      return Object.entries(error.error.errors)
        .map(([k, v]: any) => `${k}: ${v[0]}`)
        .join('\n');
    }

    return 'Erro inesperado';
  }
}