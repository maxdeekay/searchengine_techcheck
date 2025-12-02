import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, FormControl } from '@angular/forms';

@Component({
  selector: 'app-search-bar',
  standalone: false,
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.scss'
})
export class SearchBarComponent {
  @Output() search = new EventEmitter<{ query: string; engines: string[] }>();

  form: FormGroup;

  engines = ["Google", "Bing", "Wikidata"];

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      query: [""],
      selectedEngines: this.fb.array([])
    });

    const selectedEnginesArray = this.form.get("selectedEngines") as FormArray;
    this.engines.forEach(() => selectedEnginesArray.push(this.fb.control(true)));
  }

  get selectedEnginesControls() {
    return (this.form.get("selectedEngines") as FormArray).controls as FormControl[];
  }

  onSubmit() {
    const query = this.form.get("query")?.value.trim();
    if (!query) return;

    const selectedEnginesArray = this.form.get("selectedEngines") as FormArray;
    const selectedEngines = this.engines.filter((_, i) => selectedEnginesArray.at(i).value);

    if (selectedEngines.length === 0) return;

    console.log("Search: ", query);
    console.log("Engines: ", selectedEngines);

    this.search.emit({ query, engines: selectedEngines });
  }
}
