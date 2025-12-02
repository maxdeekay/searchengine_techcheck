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

export interface EngineState extends EngineResult {
  loading: boolean;
  hasErrors: boolean;
  allFailed: boolean;
}
