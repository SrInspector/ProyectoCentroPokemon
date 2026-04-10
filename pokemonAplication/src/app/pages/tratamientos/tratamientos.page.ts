import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList } from '@ionic/angular/standalone';
import { ClinicalApiService } from '../../core/services/clinical-api.service';

@Component({
  selector: 'app-tratamientos',
  templateUrl: './tratamientos.page.html',
  styleUrls: ['./tratamientos.page.scss'],
  standalone: true,
  imports: [CommonModule, IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList]
})
export class TratamientosPage implements OnInit {
  isLoading = true;
  tratamientos: any[] = [];

  constructor(private api: ClinicalApiService, private router: Router) {}

  ngOnInit() {
    this.loadTratamientos();
  }

  loadTratamientos() {
    this.isLoading = true;
    this.api.getTratamientos().subscribe({
      next: (data) => {
        this.tratamientos = data;
        this.isLoading = false;
      },
      error: () => {
        this.tratamientos = [];
        this.isLoading = false;
      }
    });
  }

  getEstadoColor(estado: string): string {
    if (estado === 'Activo') return 'warning';
    if (estado === 'Finalizado') return 'success';
    if (estado === 'Cancelado') return 'danger';
    return 'medium';
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}
