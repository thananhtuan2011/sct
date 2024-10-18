import { Pipe, PipeTransform, Injectable } from "@angular/core";

@Pipe({
  name: 'filter',
  pure: false
})
@Injectable()
export class SearchPipe implements PipeTransform {

  /**
     * @param items object from array
     * @param term term's search
     * @param excludes array of strings which will ignored during search
     */
  transform(items: any, term: string, excludes: any = []): any {
    if (!term || !items) return items;

    return SearchPipe.filter(items, term, excludes);
  }

  /**
   *
   * @param items List of items to filter
   * @param term  a string term to compare with every property of the list
   * @param excludes List of keys which will be ignored during search
   *
   */
  static filter(items: Array<{ [key: string]: any }>, term: string, excludes: any): Array<{ [key: string]: any }> {

    const toCompare = SearchPipe._normalizeValue(SearchPipe.convertVietnamese(term.toLowerCase()))

    function checkInside(item: any, term: string) {

      if (typeof item === "string" && SearchPipe._normalizeValue(SearchPipe.convertVietnamese(item.toString().toLowerCase())).includes(toCompare)) {
        return true;
      }

      for (let property in item) {
        if (item[property] === null || item[property] == undefined || excludes.includes(property)) {
          continue;
        }
        if (typeof item[property] === 'object') {
          if (checkInside(item[property], term)) {
            return true;
          }
        }
        else if (SearchPipe._normalizeValue(SearchPipe.convertVietnamese(item[property].toString().toLowerCase())).includes(toCompare)){
          return true;
        }
      }
      return false;
    }

    return items.filter(function (item) {
      return checkInside(item, term);
    });
  }
  static convertVietnamese(str: any) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    // str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'| |\"|\&|\#|\[|\]|~|$|_/g, "-");
    // str = str.replace(/-+-/g, "-"); //thay thế khoảng cách bằng ký tự
    str = str.replace(/^\-+|\-+$/g, "");
    str = str.toLowerCase().replace(/\b[a-z](?=[a-z]{2})/g, (letter: any) => { return letter.toUpperCase(); });
    return str;
  }
  static _normalizeValue(value: string): string {
    return value.toLowerCase().replace(/\s/g, '');
}
}
