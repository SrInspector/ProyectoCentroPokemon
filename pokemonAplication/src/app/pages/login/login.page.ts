import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {
  IonContent, IonItem, IonLabel, IonInput,
  IonButton, IonSpinner, IonIcon
} from '@ionic/angular/standalone';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    IonContent,
    IonItem,
    IonLabel,
    IonInput,
    IonButton,
    IonSpinner,
    IonIcon
  ]
})
export class LoginPage {

  email: string = '';
  password: string = '';
  isLoading: boolean = false;
  errorMessage: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  login() {
    this.errorMessage = '';
    this.isLoading = true;

    this.authService.login(this.email, this.password).subscribe({
      next: (response) => {
        this.authService.saveSession(response.token, response.user);
        this.isLoading = false;
        this.router.navigate(['/pokemon-list']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.message || 'Error al iniciar sesión';
      }
    });
  }
}