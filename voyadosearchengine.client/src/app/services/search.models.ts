export interface SearchRequest {
  query: string;
  engine: string;
}

export interface WordResult {
  hits?: number;
  errorMessage?: string;
}

export interface EngineResult {
  name: string;
  totalHits: number;
  wordResults: { [word: string]: WordResult };
}
