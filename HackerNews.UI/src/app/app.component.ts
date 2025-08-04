import { Component } from '@angular/core';
import { StoryListComponent } from './components/story-list/story-list.component';
import { CommonModule } from '@angular/common';
// import { LoaderComponent } from './components/loader/loader.component';

@Component({
  selector: 'app-root',
  imports: [CommonModule, StoryListComponent /*LoaderComponent*/],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'HackerNews.UI';
}
