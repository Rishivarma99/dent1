import { mkdir, writeFile } from "node:fs/promises";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";
import { StitchToolClient } from "@google/stitch-sdk";

const PROJECT_ID = "6592641313762627492";
const SCREEN_ID = "55273ff53f124be8886027995fae0c9c";
const API_KEY = process.env.STITCH_API_KEY ?? process.argv[2];

if (!API_KEY) {
  console.error("Set STITCH_API_KEY or pass API key as first argument.");
  process.exit(1);
}

const root = join(dirname(fileURLToPath(import.meta.url)), "..");
const outDir = join(
  root,
  "design",
  "stitch",
  "web-dashboard-design",
  "create-appointment-patient-search-view",
);

async function download(url, destPath) {
  const res = await fetch(url, { redirect: "follow" });
  if (!res.ok) {
    throw new Error(`Download failed ${res.status}: ${url}`);
  }
  const buf = Buffer.from(await res.arrayBuffer());
  await mkdir(dirname(destPath), { recursive: true });
  await writeFile(destPath, buf);
}

function pickUrl(obj, keys) {
  for (const key of keys) {
    const v = obj?.[key];
    if (typeof v === "string" && v.startsWith("http")) return v;
  }
  return null;
}

function findUrls(node, found = { html: null, image: null }) {
  if (!node || typeof node !== "object") return found;
  if (Array.isArray(node)) {
    for (const item of node) findUrls(item, found);
    return found;
  }
  const html = pickUrl(node, [
    "htmlDownloadUrl",
    "htmlUrl",
    "html_download_url",
    "codeDownloadUrl",
    "codeUrl",
  ]);
  const image = pickUrl(node, [
    "screenshotDownloadUrl",
    "screenshotUrl",
    "imageDownloadUrl",
    "imageUrl",
  ]);
  if (html && !found.html) found.html = html;
  if (image && !found.image) found.image = image;
  for (const value of Object.values(node)) findUrls(value, found);
  return found;
}

const client = new StitchToolClient({ apiKey: API_KEY });

try {
  const result = await client.callTool("get_screen", {
    projectId: PROJECT_ID,
    screenId: SCREEN_ID,
  });

  await mkdir(outDir, { recursive: true });
  await writeFile(
    join(outDir, "get-screen-response.json"),
    JSON.stringify(result, null, 2),
  );

  const { html, image } = findUrls(result);
  if (!html) throw new Error("No HTML download URL in get_screen response");
  if (!image) throw new Error("No screenshot download URL in get_screen response");

  await download(html, join(outDir, "screen.html"));
  await download(image, join(outDir, "screen.png"));

  console.log("Saved to:", outDir);
  console.log("  screen.html");
  console.log("  screen.png");
  console.log("  get-screen-response.json");
} finally {
  await client.close();
}
