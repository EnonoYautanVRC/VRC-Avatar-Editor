# VPM パッケージテンプレート

パッケージのビルドと公開の自動化を含む、パッケージ作成用のスターターです。

すべてのセットアップが完了したら、このリポジトリに変更をプッシュし、.zip や .unitypackage のバージョンが自動的に生成され、このパッケージのアップデートを配信する VPM で動作するリストが作成されます。いろいろなパッケージの一覧を作りたい場合は、[template-package-listing](https://github.com/vrchat-community/template-package-listing) リポジトリをチェックしてください。

## ▶ はじめに

- [![このテンプレートを使う](https://user-images.githubusercontent.com/737888/185467681-e5fdb099-d99f-454b-8d9e-0760e5a6e588.png)](https://github.com/EnonoYautanVRC/Blendshape-Copy-Extension/generate) を押してください。
  をクリックすると、このテンプレートをもとにした新しい GitHub プロジェクトが始まります。
  - 適切なリポジトリ名と説明を選びます。
  - 可視性を 'Public' に設定します。'Private'を選んで後で変更することもできます。
  - Include all branches' を選択する必要はありません。
- Git を使って、このリポジトリをローカルにクローンします。
  - Git や GitHub に慣れていない場合は、[GitHub のドキュメント](https://docs.github.com/en/get-started/quickstart/git-and-github-learning-resources) を参照してください。
- Unity Hub にフォルダを追加し、Unity プロジェクトとして開きます。
- プロジェクトを開いたら、VPM リゾルバがダウンロードされ、プロジェクトに追加されるまで待ちます。
  - これで VPM Package Maker と Package Resolver ツールにアクセスできるようになります。

## 🚇 アセットパッケージの移行

詳細は [アセットを VPM パッケージに変換する](https://vcc.docs.vrchat.com/guides/convert-unitypackage) を参照。

## ✏️ パッケージの作業

- "Packages/com.vrchat.demo-template" ディレクトリを削除するか、独自のパッケージに再利用してください。
  - パッケージを再利用する場合は、名前を変更し、生成されたメタファイルをリポジトリに追加することを忘れないでください！
- 「Packages」ディレクトリの `.gitignore` ファイルを更新して、あなたのパッケージを含めるようにします。
  - 例えば、`!com.vrchat.demo-template` を `!com.username.package-name` に変更します。
  - `.gitignore` ファイルは通常、「Packages」ディレクトリの内容を除外します。このテンプレートの `.gitignore` は、デモパッケージをどのように含めるかを示しています。これを自分のパッケージ名に変更するのは簡単です。
- Unity プロジェクトを開き、パッケージのファイルを好きなコードエディタで作業してください。
- 準備ができたら、コミットして変更をプッシュします。
- 後述するように自動化を設定したら、新しいバージョンを簡単に公開できます。

## 🤖 オートメーションのセットアップ

以下に説明する名前と値を持つリポジトリ変数を作成します。
リポジトリ変数の作成方法の詳細については、[リポジトリの設定変数の作成](https://docs.github.com/en/actions/learn-github-actions/variables#creating-configuration-variables-for-a-repository) を参照してください。
**Repository secrets**ではなく、**Repository variables**を作成していることを確認してください。

- `PACKAGE_NAME`: パッケージ名で、`com.vrchat.demo-template`のようにします。

最後に、リポジトリの 「Settings」ページに行き、「Pages」を選択し、「Build and deployment」の見出しを探します。「Source」のドロップダウンを「Deploy from a branch」から「GitHub Actions」に変更します。

これで完了です！
他にも注意点があります：

- このテンプレートの既存のフォルダ構造を維持することを強くお勧めします。
  - プロジェクトのルートは Unity プロジェクトにしてください。
  - パッケージは 「Packages」ディレクトリに置いてください。
  - このフォルダ構造から外れる場合は、24 行目、38 行目、41 行目、57 行目の 「Packages」ディレクトリにパッケージがあると仮定するパスを更新する必要があります。
- Web ファイルをルートの 「Website」 以外のフォルダに保存・生成したい場合は、[build-listing.yml](.github/workflows/build-listing.yml#L17) の`listPublicDirectory`項目を変更します。

## 🎉 リリースの公開

[Build Release](.github/workflows/release.yml) アクションを実行することでリリースを作成できます。`package.json`ファイルで指定したバージョンがリリースのバージョンとして使用されます。

## 📃 リストの再構築

手動でリリースを公開したり、手動でリリースを作成・編集・削除したりと、リリースに変更を加えるたびに、[Build Repo Listing](.github/workflows/build-listing.yml) アクションは利用可能なすべてのリリースの新しいインデックスを作成し、[GitHub Pages](https://pages.github.com/) に無料でホストされるウェブサイトとして公開します。

生成されたインデックスページは、あなたのパッケージの情報を掲載したシンプルなランディングページとして利用できます。あなたのパッケージの URL は `https://username.github.io/repo-name` という形式になります。

## 🏠 ランディングページのカスタマイズ (オプション)

リストを再構築するアクションはランディングページも公開します。<br>
このページのソースは `Website/index.html` にあります。<br>
オートメーションシステムは [Scriban](https://github.com/scriban/scriban) を使って`{{ this }}`のようなオブジェクトに最新リリースのマニフェストからの情報を入力します。既存の`{{ template.objects }}`を使って好きなところに情報を埋めてください。"Website" フォルダの内容全体が、毎回 GitHub Pages に公開されます。

## 💻 技術的なこと

自動化プロセスをあなたのニーズに合うように変更することは大歓迎です。また、私たちが採用すべきだと思う変更があれば、Pull Request を作成してください。また、採用すべき変更があれば Pull Request を作成することもできます：

### ビルドリリースアクション

[release.yml](/.github/workflows/release.yml)

これは、既存の GitHub アクションとシェルコマンドを組み合わせた複合アクションで、パッケージの .zip と .unitypackage の両方を作成します。対象のパッケージの `package.json` ファイルにある `version` という名前のリリースを作成し、zip、unitypackage、package.json ファイルをこのリリースに公開します。

### ビルドリスト

[build-listing.yml](.github/workflows/build-listing.yml)

これは、作成したリリースに基づいて vpm 互換の[Repo Listing](https://vcc.docs.vrchat.com/vpm/repos)をビルドする複合アクションです。すべてのリリースを見つけ、リストにまとめるために、[別のリポジトリ](https://github.com/vrchat-community/package-list-action)をチェックアウトします。このリポジトリには、VPM コア lib を含む[Nuke](https://nuke.build/)プロジェクトがあり、その型とメソッドにアクセスできるようになっています。このプロジェクトは将来もっと多くの機能を含むように拡張される予定です――今のところ、このアクションは `BuildRepoListing` ターゲットを呼び出すだけです。
