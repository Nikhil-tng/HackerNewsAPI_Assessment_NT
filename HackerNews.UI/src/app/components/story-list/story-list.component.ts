import { Component, OnInit } from '@angular/core';
import { HackerNewsService } from '../../services/hacker-news.service';
import { Story } from '../../models/story.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-story-list',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  providers: [HackerNewsService],
  templateUrl: './story-list.component.html',
  styleUrls: ['./story-list.component.css'],
})
export class StoryListComponent implements OnInit {
  stories: Story[] = [];
  currentPage = 1;
  pageSize = 10;
  totalItems = 0;
  totalPages = 0;
  searchTerm = '';

  constructor(private hackerNewsService: HackerNewsService) {}

  ngOnInit(): void {
    this.loadStories();
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadStories();
  }

  onClear(): void {
    this.searchTerm = '';
    this.currentPage = 1;
    this.loadStories();
  }

  getStartItem(): number {
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  getEndItem(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalItems);
  }

  onPrevious() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadStories();
    }
  }

  onNext() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadStories();
    }
  }

  getPages(): number[] {
    this.totalPages = Math.ceil(this.totalItems / this.pageSize);
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadStories();
  }

  private loadStories(): void {
    this.hackerNewsService
      .getNewStories(this.currentPage, this.pageSize, this.searchTerm)
      .subscribe((response: any) => {
        this.stories = response.result.list;
        this.totalItems = response.result.noOfRecord;
        this.getPages();
      });
  }
}
