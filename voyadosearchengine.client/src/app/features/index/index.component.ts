import { Component } from '@angular/core';
import { SearchService } from '../../services/search.service';
import { EngineResult } from '../../services/search.models';

interface EngineState {
  name: string;
  loading: boolean;
  result?: EngineResult;
  error?: string;
}

@Component({
  selector: 'app-index',
  standalone: false,
  templateUrl: './index.component.html',
  styleUrl: './index.component.scss'
})
export class IndexComponent {
  engineStates: EngineState[] = [];

  constructor(private searchService: SearchService) { }

  handleSearch(searchData: { query: string; engines: string[] }) {
    const { query, engines } = searchData;

    this.engineStates = engines.map(engine => ({
      name: engine,
      loading: true
    }));

    return;

    engines.forEach((engine, index) => {
      this.searchService.search(query, engine).subscribe({
        next: (result: EngineResult) => {
          this.engineStates[index].loading = false;
          this.engineStates[index].result = result;
        },
        error: (err: Error) => {
          console.error(`Error fetching results from ${engine}: `, err);
          this.engineStates[index].loading = false;
          this.engineStates[index].error = `Error fetching results from ${engine}`;
        }
      });
    });
  }
}
