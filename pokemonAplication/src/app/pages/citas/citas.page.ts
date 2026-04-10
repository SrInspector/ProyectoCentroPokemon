import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList } from '@ionic/angular/standalone';
import { ClinicalApiService } from '../../core/services/clinical-api.service';

@Component({
  selector: 'app-citas',
  templateUrl: './citas.page.html',
  styleUrls: ['./citas.page.scss'],
  standalone: true,
  imports: [CommonModule, IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList]
})
export class CitasPage implements OnInit {
  isLoading = true;
  citas: any[] = [];

  constructor(private api: ClinicalApiService, private router: Router) {}

  ngOnInit() {
    this.loadCitas();
  }

  loadCitas() {
    this.isLoading = true;
    this.api.getCitas().subscribe({
      next: (data) => {
        this.citas = data;
        this.isLoading = false;
      },
      error: () => {
        this.citas = [];
        this.isLoading = false;
      }
    });
  }

  getEstadoColor(estado: string): string {
    if (estado === 'Confirmada') return 'success';
    if (estado === 'Pendiente') return 'warning';
    if (estado === 'Cancelada') return 'danger';
    return 'primary';
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}
