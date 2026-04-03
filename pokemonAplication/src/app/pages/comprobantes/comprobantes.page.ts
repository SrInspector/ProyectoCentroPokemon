import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  IonHeader, IonToolbar, IonTitle, IonContent,
  IonCard, IonCardHeader, IonCardTitle, IonCardContent,
  IonBadge, IonButton, IonIcon, IonButtons,
  IonSpinner, IonItem, IonLabel, IonList
} from '@ionic/angular/standalone';
import { PokeapiService } from '../../core/services/pokeapi';
import jsPDF from 'jspdf';

@Component({
  selector: 'app-comprobantes',
  templateUrl: './comprobantes.page.html',
  styleUrls: ['./comprobantes.page.scss'],
  standalone: true,
  imports: [
    CommonModule,
    IonHeader, IonToolbar, IonTitle, IonContent,
    IonCard, IonCardHeader, IonCardTitle, IonCardContent,
    IonBadge, IonButton, IonIcon, IonButtons,
    IonSpinner, IonItem, IonLabel, IonList
  ]
})
export class ComprobantesPage implements OnInit {

  isLoading: boolean = true;
  comprobantes: any[] = [];

  mockComprobantes = [
    {
      id: 'COMP-001',
      pokemonName: 'pikachu',
      tipo: 'Consulta general',
      fecha: '2026-03-15',
      monto: 5000,
      estado: 'Pagado',
      descripcion: 'Revisión general y vacunas'
    },
    {
      id: 'COMP-002',
      pokemonName: 'charmander',
      tipo: 'Terapia física',
      fecha: '2026-03-20',
      monto: 8500,
      estado: 'Pagado',
      descripcion: 'Sesión de terapia física por lesión en cola'
    },
    {
      id: 'COMP-003',
      pokemonName: 'bulbasaur',
      tipo: 'Medicamentos',
      fecha: '2026-03-25',
      monto: 3200,
      estado: 'Pendiente',
      descripcion: 'Vitaminas y suplementos'
    }
  ];

  constructor(
    private pokeapiService: PokeapiService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadComprobantes();
  }

  loadComprobantes() {
    this.isLoading = true;
    const requests = this.mockComprobantes.map(c =>
      this.pokeapiService.getPokemonByName(c.pokemonName).toPromise()
    );

    Promise.all(requests).then(results => {
      this.comprobantes = results.map((data: any, index) => ({
        ...this.mockComprobantes[index],
        pokemonImage: data.sprites.other['official-artwork'].front_default
      }));
      this.isLoading = false;
    });
  }

  getEstadoColor(estado: string): string {
    switch (estado) {
      case 'Pagado': return 'success';
      case 'Pendiente': return 'warning';
      case 'Cancelado': return 'danger';
      default: return 'medium';
    }
  }

  descargarPDF(comprobante: any) {
    const doc = new jsPDF();

    // Header
    doc.setFillColor(204, 0, 0);
    doc.rect(0, 0, 210, 40, 'F');
    doc.setTextColor(255, 255, 255);
    doc.setFontSize(22);
    doc.setFont('helvetica', 'bold');
    doc.text('Centro Pokemon', 105, 18, { align: 'center' });
    doc.setFontSize(12);
    doc.setFont('helvetica', 'normal');
    doc.text('Comprobante de Servicio', 105, 30, { align: 'center' });

    // Referencia
    doc.setTextColor(0, 0, 0);
    doc.setFontSize(11);
    doc.setFont('helvetica', 'bold');
    doc.text(`Referencia: ${comprobante.id}`, 20, 55);
    doc.text(`Fecha: ${comprobante.fecha}`, 20, 65);

    // Linea separadora
    doc.setDrawColor(204, 0, 0);
    doc.setLineWidth(0.5);
    doc.line(20, 72, 190, 72);

    // Detalles
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(13);
    doc.text('Detalles del Servicio', 20, 85);

    doc.setFont('helvetica', 'normal');
    doc.setFontSize(11);
    doc.text(`Pokemon: ${comprobante.pokemonName.toUpperCase()}`, 20, 98);
    doc.text(`Tipo de servicio: ${comprobante.tipo}`, 20, 110);
    doc.text(`Descripcion: ${comprobante.descripcion}`, 20, 122);

    // Linea separadora
    doc.line(20, 130, 190, 130);

    // Monto
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(14);
    doc.text('Total a pagar:', 20, 145);
    doc.setTextColor(204, 0, 0);
    doc.setFontSize(18);
    doc.text(`CRC ${comprobante.monto.toLocaleString()}`, 105, 145, { align: 'center' });

    // Estado
    doc.setTextColor(0, 0, 0);
    doc.setFontSize(12);
    doc.text(`Estado: ${comprobante.estado}`, 20, 160);

    // Footer
    doc.setFillColor(204, 0, 0);
    doc.rect(0, 270, 210, 30, 'F');
    doc.setTextColor(255, 255, 255);
    doc.setFontSize(10);
    doc.text('Centro Pokemon - Todos los derechos reservados', 105, 282, { align: 'center' });
    doc.text('Tel: 2222-3333 | centropokemon@mail.com', 105, 290, { align: 'center' });

    doc.save(`${comprobante.id}.pdf`);
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}