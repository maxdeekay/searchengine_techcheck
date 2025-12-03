import { Component, OnInit } from '@angular/core';
import { SearchService } from '../../services/search.service';
import { EngineResult, EngineState } from '../../services/search.models';

@Component({
  selector: 'app-index',
  standalone: false,
  templateUrl: './index.component.html',
  styleUrl: './index.component.scss'
})
export class IndexComponent implements OnInit {
  engineStates: EngineState[] = [];
  availableEngines: string[] = [];

  constructor(private searchService: SearchService) { }

  ngOnInit() {
    this.searchService.getEngines().subscribe({
      next: (engines) => {
        this.availableEngines = engines;
      },
      error: (err: Error) => {
        console.error("Error fetching engines: ", err);
      }
    });
  }

  handleSearch(searchData: { query: string; engines: string[] }) {
    const { query, engines } = searchData;

    this.engineStates = engines.map(engine => ({
      name: engine,
      loading: true,
      totalHits: 0,
      wordResults: {},
      hasErrors: false,
      allFailed: false
    }));

    engines.forEach((engine, index) => {
      this.searchService.search(query, engine).subscribe({
        next: (result: EngineResult) => {
          console.log("Result: ", result);

          const hasErrors = Object.values(result.wordResults).some(wr => wr.errorMessage);
          const allFailed = Object.values(result.wordResults).every(wr => wr.errorMessage);

          this.engineStates[index] = {
            ...result,
            loading: false,
            hasErrors,
            allFailed
          };
        },
        error: (err: Error) => {
          console.error(`Error fetching results from ${engine}: `, err);
          this.engineStates[index].loading = false;
          this.engineStates[index].hasErrors = true;
          this.engineStates[index].allFailed = true;
        }
      });
    });
  }

  formatNumber(num: number): string {
    if (num >= 1000000) {
      return (num / 1000000).toFixed(2).replace(/\.?0+$/, '') + 'M';
    } else if (num >= 1000) {
      return (num / 1000).toFixed(2).replace(/\.?0+$/, '') + 'K';
    }
    return num.toString();
  }
}
