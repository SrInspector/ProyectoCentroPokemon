import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel } from '@ionic/angular/standalone';
import { ClinicalApiService } from '../../core/services/clinical-api.service';

@Component({
  selector: 'app-pokemon-detail',
  templateUrl: './pokemon-detail.page.html',
  styleUrls: ['./pokemon-detail.page.scss'],
  standalone: true,
  imports: [CommonModule, IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel]
})
export class PokemonDetailPage implements OnInit {
  pokemon: any = null;
  isLoading = true;
  pokemonId = 0;

  constructor(private route: ActivatedRoute, private router: Router, private api: ClinicalApiService) {}

  ngOnInit() {
    this.pokemonId = Number(this.route.snapshot.paramMap.get('id') || 0);
    this.loadPokemon();
  }

  loadPokemon() {
    this.api.getPokemon(this.pokemonId).subscribe({
      next: (data) => {
        this.pokemon = data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}
