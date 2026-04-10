import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList } from '@ionic/angular/standalone';
import { ClinicalApiService } from '../../core/services/clinical-api.service';

@Component({
  selector: 'app-comprobantes',
  templateUrl: './comprobantes.page.html',
  styleUrls: ['./comprobantes.page.scss'],
  standalone: true,
  imports: [CommonModule, IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList]
})
export class ComprobantesPage implements OnInit {
  isLoading = true;
  comprobantes: any[] = [];

  constructor(private api: ClinicalApiService, private router: Router) {}

  ngOnInit() {
    this.loadComprobantes();
  }

  loadComprobantes() {
    this.isLoading = true;
    this.api.getFacturas().subscribe({
      next: (data) => {
        this.comprobantes = data;
        this.isLoading = false;
      },
      error: () => {
        this.comprobantes = [];
        this.isLoading = false;
      }
    });
  }

  getEstadoColor(estado: string): string {
    if (estado === 'Pagada') return 'success';
    if (estado === 'Pendiente') return 'warning';
    if (estado === 'Cancelada') return 'danger';
    return 'medium';
  }

  descargarPDF(comprobante: any) {
    this.api.downloadComprobante(comprobante.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `comprobante-${comprobante.id}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      }
    });
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}
