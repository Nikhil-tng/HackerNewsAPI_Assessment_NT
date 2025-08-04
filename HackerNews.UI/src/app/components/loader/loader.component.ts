import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { LoaderService } from '../../services/loader.service';

@Component({
  selector: 'app-loader',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="loader-overlay" *ngIf="loaderService.loading$ | async">
      <div class="spinner">Loading...</div>
    </div>
  `,
  styleUrls: ['./loader.component.css'],
})
export class LoaderComponent {
  constructor(public loaderService: LoaderService) {}
}
