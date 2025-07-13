import { Component, OnInit } from '@angular/core';
import { TokenDecodeService } from './shared/services/token-decode.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit{
  constructor(private auth : TokenDecodeService) {}

  ngOnInit() {
    this.auth.initialize();
  }

  title = 'OnlineStoreUI';
}
